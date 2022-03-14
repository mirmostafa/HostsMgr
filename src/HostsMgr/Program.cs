using System.Data;
using System.Security.Principal;
using Windows.Management;

var rules = SystemHostsFileMaanger.ReadRules();
SystemHostsFileMaanger.WriteRules(rules);

bool IsRunAsAdmin()
{
    var id = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(id);

    return principal.IsInRole(WindowsBuiltInRole.Administrator);
}

namespace Windows.Management
{
    public static class SystemHostsFileMaanger
    {
        private static readonly string _filePath = @"C:\Windows\System32\drivers\etc\hosts";

        public static IEnumerable<(string Ip, string Site, string? Description)> ReadRules() =>
            from rule in File.ReadAllText(_filePath).Split(Environment.NewLine)
            where !string.IsNullOrEmpty(rule) && !rule.StartsWith("#")
            let description = rule.IndexOf("#") > 0 ? rule[(rule.IndexOf("#") + 1)..] : null
            let pair = rule.Split(" ")
            let ip = pair[0].Trim()
            let site = pair[1].Trim()
            select (ip, site, description);

        public static void WriteRules(IEnumerable<(string Ip, string Site, string? Description)> rules) =>
            File.WriteAllLines(_filePath, rules.Select(rule => string.Concat($"{rule.Ip} {rule.Site}", !string.IsNullOrEmpty(rule.Description) ? $" #{rule.Description}" : null)));
    }
}