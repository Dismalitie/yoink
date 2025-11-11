namespace yoink
{
    internal interface ICommand
    {
        public static abstract void Invoke(string[] args);
        public static abstract void Help();
    }
}
