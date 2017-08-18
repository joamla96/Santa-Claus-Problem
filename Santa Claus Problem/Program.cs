using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Santa_Claus_Problem
{
	class Program
	{
		int elfCount = 0;
		int reindeerCount = 0;

		Semaphore santaSem = new Semaphore(0, 1);
		Semaphore reindeerSem = new Semaphore(0, 9);
		Semaphore elfSem = new Semaphore(0, 3);

		Mutex m = new Mutex();
		Mutex elfMutex = new Mutex();

		List<Thread> Elves = new List<Thread>();
		List<Thread> Reindeers = new List<Thread>();

		public static bool programRunning = true;

		Random rnd = new Random();

		static void Main(string[] args)
		{
			Program program = new Program();
			program.Run();
		}

		private void Run()
		{
			Console.WriteLine("Starting Application");

			Thread santa = new Thread(Santa);

			santa.Name = "Santa";

			santa.Start();
			//reindeer.Start();

			for(int i = 0; i < 9; i++)
			{
				//SpawnReindeer();
			}

			while(programRunning)
			{
				switch(Console.ReadKey().Key)
				{
					case ConsoleKey.E: SpawnElf(); break;
					//case ConsoleKey.R: SpawnReindeer(); break;
				}
			}

			Console.ReadKey();
		}

		private void SpawnElf()
		{
			Thread t = new Thread(Elf);
			t.Start();
			Elves.Add(t);
		}

		private void SpawnReindeer()
		{
			Thread t = new Thread(Reindeer);
			t.Start();
			Reindeers.Add(t);
		}

		private void Reindeer()
		{
			Console.WriteLine("Reindeer going on holiday");
			Thread.Sleep(rnd.Next(100, 1500)); // Wait for reindeer to return from holiday, at random.

			Console.WriteLine("Reindeer came home & waiting for mutex");
			m.WaitOne();
			Console.WriteLine("Reindeer got mutex");
			this.reindeerCount++;
			if (this.reindeerCount == 9)
			{
				Console.WriteLine("9 Reindeers, Wake Santa");
				santaSem.Release();
			}
			m.ReleaseMutex();
			Console.WriteLine("Reindeer releases mutex");

			reindeerSem.WaitOne();
			getHitched();
		}

		private void getHitched()
		{
			Console.WriteLine("Reindeer getting Hitched");
			m.WaitOne();
			Console.WriteLine("Reindeer got mutex");
			reindeerCount--;
			if (reindeerCount == 0)
			{
				Console.WriteLine("All Reindeers hitched, release mutex");
				reindeerSem.Release(9);
			}
			m.ReleaseMutex();
		}

		private void Elf()
		{
			Console.WriteLine("Elf waiting for elfmutex and mutex");
			elfMutex.WaitOne();
			m.WaitOne();
			Console.WriteLine("Elf got mutexes");

			elfCount++;
			if (elfCount == 3)
			{
				Console.WriteLine("3 Elves in line, release santa");
				santaSem.Release();
			}
			else
			{
				Console.WriteLine("Not 3 elves in line, release elfmutex");
				elfMutex.ReleaseMutex();
			}

			m.ReleaseMutex();
			elfSem.WaitOne();

			getHelp();

			m.WaitOne();
			elfCount--;
			if (elfCount == 0)
			{
				Console.WriteLine("All Elves got help, release elfmutex");
				elfMutex.ReleaseMutex();
			}
			m.ReleaseMutex();
		}

		private void getHelp()
		{
			Thread.Sleep(100);
		}

		private void Santa()
		{
			while (programRunning)
			{
				Console.WriteLine("Santa going to Sleep");
				santaSem.WaitOne();
				m.WaitOne();
				if (reindeerCount == 9)
				{
					Console.WriteLine("Santa got 9 reindeers.");
					elfSem.Release(3);
					reindeerCount = 0;
					prepSleigh();
					reindeerSem.Release(9);
				}
				else
				{
					Console.WriteLine("Santa got 3 elves");
					helpElves();
					elfSem.Release(3);
				}
				m.ReleaseMutex();
			}
		}

		private void prepSleigh()
		{
			Thread.Sleep(500);
		}

		private void helpElves()
		{
			Console.WriteLine("Santa Helping Elves");
			Thread.Sleep(100);
		}
	}
}
