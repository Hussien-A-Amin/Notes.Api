using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Chatting.Api.Application.Services;

public static class ApplicationStaticSettings
{
    internal static void SetGlobalStaticValues(ApplicationSettings applicationSettings)
    {
        
    }
}
public class ApplicationSettings
{

    public int ProductsCount { get; set; }
    public int VendorsCount { get; set; }

    
    internal void SetGlobalDeletePeriod()
    {
        ApplicationStaticSettings.SetGlobalStaticValues(this);
    }
}
public class ApplicationSettingsManager
{
    private readonly IOptionsMonitor<ApplicationSettings> _optionsMonitor;
    private readonly string _configFilePath;
    private static readonly object _fileLock = new(); // لمنع حدوث التضارب (Race Condition) أثناء القراءة والكتابة

    public ApplicationSettingsManager(IOptionsMonitor<ApplicationSettings> optionsMonitor, IWebHostEnvironment env)
    {
        _optionsMonitor = optionsMonitor;
        // تأكد من اسم الملف المستقل
        _configFilePath = Path.Combine(env.ContentRootPath, "appsettings.system.json");

        _optionsMonitor.CurrentValue?.SetGlobalDeletePeriod();



        _optionsMonitor.OnChange(p => p.SetGlobalDeletePeriod());



    }
    internal ApplicationSettings GetSettings()
    {
        return _optionsMonitor.CurrentValue;
    }
    // تم حذف كلمة Async لأن الميثود شغالة بنظام الـ Blocking الـ عادي


    // جعلنا الميثود تقبل Action لتحديث البيانات بأمان تام جوه الـ Lock
    private void UpdateSettings(Action<ApplicationSettings> updateAction)
    {
        lock (_fileLock)
        {
            // 1. جلب القيمة اللحظية الحالية الفريش من الـ Monitor جوة الـ Lock
            var currentSettings = _optionsMonitor.CurrentValue;

            // 2. تطبيق التعديل (الزيادة أو النقصان)
            updateAction(currentSettings);

            // 3. قراءة ملف الـ JSON الحالي
            string jsonText = File.ReadAllText(_configFilePath);
            var rootNode = JsonNode.Parse(jsonText);

            if (rootNode is JsonObject jsonObject)
            {
                // 4. عمل Serialize للكائن بعد تعديله
                var updatedSettingsNode = JsonSerializer.SerializeToNode(currentSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // 5. تحديث السكشن الخاص بنا فقط داخل الـ JSON
                jsonObject["ApplicationSettings"] = updatedSettingsNode;

                // 6. الحفظ الفعلي في الملف
                File.WriteAllText(_configFilePath, jsonObject.ToString());
            }
        }
    }

    //internal void ChangeGeneralSettings(GeneralSettings md)
    //{
    //    UpdateSettings(settings => settings.GeneralSettings = md);

    //}


}
