using System.Collections.ObjectModel;

namespace His_Pos.NewClass.Person
{
    public class Genders : ObservableCollection<Gender>
    {
        public Genders()
        {
            Add(new Gender("男"));
            Add(new Gender("女"));
        }
    }
}