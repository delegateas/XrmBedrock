namespace ConsoleJobs;

public interface IJob
{
    void Run(JobContext ctx);
}
