namespace His_Pos.Interface
{
    public interface IPrincipal
    {
        string Id { get; set; }
        string Name { get; set; }
        string NickName { get; set; }
        string Telephone { get; set; }
        string Fax { get; set; }
        string Email { get; set; }
        string Line { get; set; }
        bool IsEnable { get; set; }
    }
}