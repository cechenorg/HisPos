using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Tutorial;

namespace His_Pos.SYSTEM_TAB.H9_SYSTEMTUTORIAL.Tutorial
{
    public class TutorialViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        public Tutorials tutorialCollection;

        public Tutorials TutorialCollection
        {
            get { return tutorialCollection; }
            set
            {
                Set(() => TutorialCollection, ref tutorialCollection, value);
            }
        }

        public NewClass.Tutorial.Tutorial tutorialSelectedItem;

        public NewClass.Tutorial.Tutorial TutorialSelectedItem
        {
            get { return tutorialSelectedItem; }
            set
            {
                Set(() => TutorialSelectedItem, ref tutorialSelectedItem, value);
            }
        }

        public TutorialViewModel()
        {
            TutorialCollection = Tutorials.GetData();
        }
    }
}