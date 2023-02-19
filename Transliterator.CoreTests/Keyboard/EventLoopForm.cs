using Moq;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Services;

namespace Transliterator.CoreTests.Keyboard
{
    public partial class EventLoopForm : Form
    {
        public string keyboardHookMemory = "";
        private Mock<BufferedTransliteratorService> mock;
        public BufferedTransliteratorService bufferedTransliteratorService;


        private KeyboardHook _keyboardHook;

        public EventLoopForm()
        {
            Width = 0;
            Height = 0;
            Visible = false;

            _keyboardHook = new KeyboardHook();

            _keyboardHook.SkipUnicodeKeys = false;
            InitializeComponent();
        }

        // TODO: Rename
        public void AttachKeyboardHook()
        {
            _keyboardHook.KeyDown += KeyPressedHandler;
        }

        public void AttachBufferedTransliteratorService()
        {
            //bufferedTransliteratorService = BufferedTransliteratorService.GetInstance();

            // replace whatever the translit is using to output results with mock function
            // which will simply store the results in a variable
            mock = new Mock<BufferedTransliteratorService>();
            bufferedTransliteratorService = mock.Object;
            // state may depend on settings or whatever, so better set it to true here
            bufferedTransliteratorService.State = true;

            mock.Setup(foo => foo.EnterTransliterationResults(It.IsAny<string>())).Returns((string s) => keyboardHookMemory += s);
        }

        private void EventLoopForm_Load(object sender, EventArgs e)
        {
        }

        private void KeyPressedHandler(object? sender, KeyboardHookEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            keyboardHookMemory += e.Character;
            e.Handled = true;
        }
    }
}