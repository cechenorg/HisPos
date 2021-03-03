namespace His_Pos.NewClass.Setting
{
    public struct SettingTabData
    {
        public SettingTabData(SettingTabs tab, string name, string icon)
        {
            Tab = tab;
            Name = name;
            Icon = icon;
        }

        public SettingTabs Tab { get; }
        public string Name { get; }
        public string Icon { get; }
    }
}