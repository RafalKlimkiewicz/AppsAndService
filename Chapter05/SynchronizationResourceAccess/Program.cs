

using System.Diagnostics;

WriteLine("Please wait for the tasks to complete.");

var watch = Stopwatch.StartNew();

Task a = Task.Factory.StartNew(MethodA);
Task b = Task.Factory.StartNew(MethodB);

Task.WaitAll(new Task[] { a, b });

WriteLine();
WriteLine($"Results: {SharedObjects.Messsage}");
WriteLine($"{watch.ElapsedMilliseconds:N0} elapsed milliseconds");
