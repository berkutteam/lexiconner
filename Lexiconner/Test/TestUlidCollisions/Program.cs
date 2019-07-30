using NUlid;
using NUlid.Rng;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestUlidCollisions
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const int TaskCount = 10;
            const int LoopCount = 5000;

            // Ulid
            var tasks = Enumerable.Range(0, TaskCount).Select(x =>
            {
                return Task.Run(() =>
                {
                    List<string> result = new List<string>();
                    for (int i = 0; i < LoopCount; i++)
                    {

                        string id = Ulid.NewUlid(new  SimpleUlidRng()).ToString(); // has collisions
                        // string id = Ulid.NewUlid(new NUlid.Rng.CSUlidRng()).ToString(); // no collisions
                        //string id = Ulid.NewUlid(new NUlid.Rng.MonotonicUlidRng()).ToString(); // no collisions
                        result.Add(id);
                    }
                    return result;
                });
            }).ToList();

            var results = Task.WhenAll<List<string>>(tasks).GetAwaiter().GetResult();
            List<string> allIds = results.SelectMany(x => x).ToList();

            int distincCount = allIds.Distinct().Count();
            bool hasCollisions = allIds.Count() != distincCount;
            Console.WriteLine($"ULid. Has collisions: {hasCollisions}. totalCount = {allIds.Count()}, distincCount = {distincCount}");

            // Guid
            var tasks2 = Enumerable.Range(0, TaskCount).Select(x =>
            {
                return Task.Run(() =>
                {
                    List<string> result = new List<string>();
                    for (int i = 0; i < LoopCount; i++)
                    {
                        string id = Guid.NewGuid().ToString();
                        result.Add(id);
                    }
                    return result;
                });
            }).ToList();

            var results2 = Task.WhenAll<List<string>>(tasks2).GetAwaiter().GetResult();
            List<string> allIds2 = results2.SelectMany(x => x).ToList();

            int distincCount2 = allIds2.Distinct().Count();
            bool hasCollisions2 = allIds2.Count() != distincCount2;
            Console.WriteLine($"Guid. Has collisions: {hasCollisions2}. totalCount = {allIds2.Count()}, distincCount = {distincCount2}");

            Console.ReadKey();
        }

    }
}
