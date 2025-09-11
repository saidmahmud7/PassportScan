using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Domain.Dtos;
using Domain.Dtos.Passport;
using Domain.Entities;
using Infrastructure.Interface;
using Infrastructure.Repositories.PassportRepositories;
using Infrastructure.Response;
using IronOcr;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Service
{
    public class PassportService(
        IPassportRepository repository,
        ILogger<PassportService> logger,     
        IHostEnvironment environment)
        : IPassportService
    {
        private static readonly CultureInfo RussianCulture = new CultureInfo("ru-RU");

        public async Task<ApiResponse<List<PassportDto>>> GetAllAsync()
        {
            var passports = await repository.GetPassportsAsync();
            var result = passports.Select(p => new PassportDto
            {
                Id = p.Id,
                Data = CleanPassportText(p.Data),
                FilePath = p.FilePath,
                CreatedAt = p.CreatedAt,
            }).ToList();

            return new ApiResponse<List<PassportDto>>(result);
        }

        public async Task<ApiResponse<string>> ProcessPdfAsync(IFormFile file)
        {
            var license = Environment.GetEnvironmentVariable("Key_Ocr");
            License.LicenseKey = license;
            
            if (file == null || file.Length == 0)
                return new ApiResponse<string>(HttpStatusCode.BadRequest, "Файл отсутствует или пуст.");

            var validExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(fileExtension))
                return new ApiResponse<string>(HttpStatusCode.BadRequest, "Ожидается файл формата PDF, PNG или JPG.");

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadsFolder = Path.Combine(environment.ContentRootPath, "uploads", "passports");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                logger.LogInformation("Создание директории: {UploadsFolder}", uploadsFolder);
                Directory.CreateDirectory(uploadsFolder);

                logger.LogInformation("Сохранение файла в: {FilePath}", filePath);
                await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                if (!File.Exists(filePath))
                {
                    logger.LogError("Файл не был сохранен по пути: {FilePath}", filePath);
                    return new ApiResponse<string>(HttpStatusCode.InternalServerError, "Не удалось сохранить файл.");
                }
                
                Installation.LanguagePackDirectory = "/app/tessdata_best";

                string fullText;
                var ocr = new IronTesseract
                {
                    Language = OcrLanguage.TajikBest,
                };

                try
                {
                    ocr.AddSecondaryLanguage(OcrLanguage.EnglishBest);
                }
                catch (IronOcr.Exceptions.LanguagePackException ex)
                {
                    logger.LogError(ex,
                        "Не удалось загрузить языковой пакет. Убедитесь, что установлен пакет IronOcr.Languages.Tajik.");
                    return new ApiResponse<string>(HttpStatusCode.InternalServerError,
                        "Ошибка: отсутствует языковой пакет. Установите IronOcr.Languages.Tajik или проверьте настройки.");
                }

                using (var input = new OcrInput())
                {
                    if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        input.LoadPdf(filePath, 300);
                        
                    }
                    else
                    {
                        input.AddImage(filePath);
                    }

                    input.DeNoise(true);
                    input.Deskew();
                    input.Contrast();


                    var results = ocr.Read(input);
                    fullText = results.Text;
                    logger.LogInformation("Извлеченный текст: {FullText}", fullText);
                }

                var cleanedText = CleanPassportText(fullText);

                var entity = new Passport
                {
                    Data = cleanedText,
                    FullText = fullText,
                    FilePath = $"/uploads/passports/{uniqueFileName}",
                    CreatedAt = DateTime.UtcNow
                };

                var result = await repository.AddAsync(entity);
                logger.LogInformation("Repository AddAsync returned: {Result}, FilePath: {FilePath}", result,
                    entity.FilePath);

                if (result == 1) return new ApiResponse<string>(HttpStatusCode.OK, "Success");
                logger.LogError("Не удалось сохранить данные паспорта в репозитории. FilePath: {FilePath}",
                    entity.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return new ApiResponse<string>(HttpStatusCode.InternalServerError,
                    "Не удалось сохранить данные паспорта.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Произошла ошибка при обработке файла: {Message}", ex.Message);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return new ApiResponse<string>(HttpStatusCode.InternalServerError,
                    $"Ошибка при обработке файла: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> UpdatePassportAsync(int id, PassportDto dto)
        {
            // var passport = await repository.GetByIdAsync(p => p.Id == id);
            // if (passport == null)
            //     return new ApiResponse<string>(HttpStatusCode.NotFound, "Passport Not Found");
            //
            // passport.Data = dto.Data;
            throw new Exception("Loading");

        }


        public async Task<ApiResponse<PassportDto>> GetPassportAsync(int id)
        {
            var passport = await repository.GetByIdAsync(p => p.Id == id);
            if (passport == null)
                return new ApiResponse<PassportDto>(HttpStatusCode.NotFound, "Паспорт не найден");

            var dto = new PassportDto
            {
                Id = passport.Id,
                Data = CleanPassportText(passport.Data),
                FilePath = passport.FilePath
            };

            return new ApiResponse<PassportDto>(dto);
        }

        public async Task<ApiResponse<string>> DeletePassportAsync(int id)
        {
            var passport = await repository.GetByIdAsync(p => p.Id == id);
            if (passport == null)
            {
                return new ApiResponse<string>(HttpStatusCode.NotFound, "Паспорт не найден");
            }

            var filePath = Path.Combine(environment.ContentRootPath, passport.FilePath.TrimStart('/'));
            var textFilePath = Path.Combine(environment.ContentRootPath, "uploads", "passports",
                $"{Path.GetFileNameWithoutExtension(passport.FilePath)}.txt");

            var result = await repository.DeleteAsync(passport);
            if (result == 1)
            {   
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        logger.LogInformation("Файл удален: {FilePath}", filePath);
                    }
                    else
                    {
                        logger.LogWarning("Файл не найден по пути: {FilePath}", filePath);
                    }

                    if (File.Exists(textFilePath))
                    {
                        File.Delete(textFilePath);
                        logger.LogInformation("Текстовый файл удален: {TextFilePath}", textFilePath);
                    }
                    else
                    {
                        logger.LogWarning("Текстовый файл не найден по пути: {TextFilePath}", textFilePath);
                    }

                    return new ApiResponse<string>(HttpStatusCode.OK,"Success");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при удалении файлов: {Message}", ex.Message);
                    return new ApiResponse<string>(HttpStatusCode.OK,
                        $"Запись удалена, но произошла ошибка при удалении файлов: {ex.Message}");
                }
            }

            logger.LogError("Не удалось удалить запись паспорта из репозитория. ID: {Id}", id);
            return new ApiResponse<string>(HttpStatusCode.BadRequest, "Failed");
        }

        #region Helpers

        private static string ExtractOneOf(string text, params string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (match.Success)
                    return match.Groups.Count > 1 ? match.Groups[1].Value.Trim() : match.Value.Trim();
            }

            return string.Empty;
        }

        private static string CleanPassportText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = Regex.Replace(text, @"[^\w\s\r\nа-яА-Яa-zA-Z0-9.,:/()-]", " ");
            text = Regex.Replace(text, @"\s+", " ");
            text = Regex.Replace(text, @"(\r\n){3,}", "\r\n\r\n");
            text = text.Remove(0,5);
            text = Regex.Replace(text, @"^\s*[^a-zA-Zа-яА-Я0-9]+$", "", RegexOptions.Multiline);

            return text.Trim();
        }

        #endregion
    }
}