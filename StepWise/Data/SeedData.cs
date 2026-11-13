using StepWise.Models;

namespace StepWise.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Instructions.Any()) return;

        db.Instructions.AddRange(
            new Instruction
            {
                Title = "Установка Git",
                Description = "Как установить Git и сделать первый коммит.",
                Category = "Разработка",
                AuthorId = "seed",
                Steps = new List<Step>
                {
                    new() { Order = 1, Title = "Скачать установщик", Description = "Откройте git-scm.com и скачайте версию для Windows." },
                    new() { Order = 2, Title = "Запустить установку", Description = "Оставьте настройки по умолчанию." },
                    new() { Order = 3, Title = "Проверить версию", Description = "В терминале выполните git --version." },
                    new() { Order = 4, Title = "Настроить имя", Description = "Выполните git config --global user.name \"Ваше имя\"." }
                }
            },
            new Instruction
            {
                Title = "Заварить чай",
                Description = "Простая инструкция для демонстрации.",
                Category = "Быт",
                AuthorId = "seed",
                Steps = new List<Step>
                {
                    new() { Order = 1, Title = "Вскипятить воду", Description = "Налейте воду в чайник и включите его." },
                    new() { Order = 2, Title = "Положить чай", Description = "Одна ложка на кружку." },
                    new() { Order = 3, Title = "Залить водой", Description = "Подождите 3–5 минут." }
                }
            },
            new Instruction
            {
                Title = "Первое резюме",
                Description = "Как составить резюме без опыта работы.",
                Category = "Карьера",
                AuthorId = "seed",
                Steps = new List<Step>
                {
                    new() { Order = 1, Title = "Выбрать шаблон", Description = "Лучше один лист." },
                    new() { Order = 2, Title = "Описать навыки", Description = "Только то, что реально умеете." },
                    new() { Order = 3, Title = "Добавить проекты", Description = "Ссылки на GitHub или учебные работы." },
                    new() { Order = 4, Title = "Проверить ошибки", Description = "Прочитайте вслух." }
                }
            });

        db.SaveChanges();
    }
}