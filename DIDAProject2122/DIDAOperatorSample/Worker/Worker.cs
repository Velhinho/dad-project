using System;

namespace Worker {
    class Worker {
        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            Console.ReadLine();

        }
    }
}
