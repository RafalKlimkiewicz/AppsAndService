using System.Diagnostics;

OutputThreadInfo();

var timer = Stopwatch.StartNew();

//SectionTitle("Running methods synchronously on one thread.");

//MethodA();
//MethodB();
//MethodC();

/////////////////// sync
//SectionTitle("Running methods asynchronously on multiple thread.");

//Task taskA = new(MethodA);

//taskA.Start();
//Task taskB  = Task.Factory.StartNew(MethodB);
//Task taskC = Task.Run(MethodC);

//Task[] tasks = { taskA, taskB, taskC };

//Task.WaitAll(tasks);


//WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed");

/////////////////// async
//SectionTitle("Passing the result of one task as an input into another.");

//Task<string> taskServiceThensProc = Task.Factory.StartNew(CallWebService)
//    .ContinueWith(previousTask =>
//    CallStoredProcedure(previousTask.Result));

//WriteLine($"{taskServiceThensProc.Result}");
//WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed");


/////////////////// inner task
SectionTitle("Nested and child tasks");

var outerTask = Task.Factory.StartNew(OuterTask);

outerTask.Wait();

WriteLine("Console app is stopping.");
