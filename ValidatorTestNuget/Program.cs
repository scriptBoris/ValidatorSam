namespace ValidatorTestNuget
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Programm is running!");

            var mock = new MockClass();
            bool result = mock.CheckValid();
            if (result)
            {
                Console.WriteLine("MockClass is valid");
            }
            else
            {
                Console.WriteLine("MockClass is invalid");
            }

            Console.ReadLine();
        }
    }
}
