namespace Sleddog.BigRedButton
{
    public struct BigRedButtonState
    {
        public bool IsLidOpen { get; private set; }
        public bool IsButtonPressed { get; private set; }

        public BigRedButtonState(bool isButtonPressed, bool isLidOpen) : this()
        {
            IsButtonPressed = isButtonPressed;
            IsLidOpen = isLidOpen;
        }

        public override string ToString()
        {
            return string.Format("Button: {0} - Lid: {1}", IsButtonPressed, IsLidOpen);
        }
    }
}