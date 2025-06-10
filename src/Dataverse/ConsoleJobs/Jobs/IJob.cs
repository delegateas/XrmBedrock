using ConsoleJobs.Setup;

namespace ConsoleJobs.Jobs;

internal interface IJob
{
    void Run(EnvironmentConfig env);
}