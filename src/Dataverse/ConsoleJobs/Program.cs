using ConsoleJobs.Jobs;
using ConsoleJobs.Setup;
using System.Configuration;
using Environment = ConsoleJobs.Setup.Environment;

namespace ConsoleJobs;

internal static class Program
{
    private static void Main()
    {
        var env = GetEnviromentFromConfig();
        IJob job = GetJobFromConfiguration(); // Used to get the job from the app config file
        ExecuteJobOnEnvironment(env, job);
    }

    /// <summary>
    /// Gets the enviroment from app config.
    /// </summary>
    /// <returns>The Dataverse environment to execute the job on</returns>
    private static Environment GetEnviromentFromConfig()
    {
        Environment result;
        var envStr = ConfigurationManager.AppSettings["Environment"];
        if (string.IsNullOrWhiteSpace(envStr) || !Enum.TryParse(envStr, out result))
        {
            throw new ConfigurationErrorsException("Environment not specified in App.config or could not be parsed as EnvironmentEnum");
        }

        return result;
    }

    /// <summary>
    /// Creates an instance of the IJob given the classname.
    /// </summary>
    /// <returns>The job to execute</returns>
    private static IJob GetJobFromConfiguration()
    {
        var envStr = ConfigurationManager.AppSettings["JobClassName"];
        if (string.IsNullOrWhiteSpace(envStr))
        {
            throw new ConfigurationErrorsException("JobClassName not specified in App.config");
        }

        Type t = Type.GetType(envStr);
        return (IJob)Activator.CreateInstance(t);
    }

    /// <summary>
    /// Creates a connection to CRM by getting the clientid and secret from the app config.
    /// </summary>
    /// <param name="env">Which Dataverse environment to execute on</param>
    /// <returns>Returns the config with relevant properties for the job, including the Dao</returns>
    private static EnvironmentConfig GetEnv(Environment env)
    {
        var newEnv = EnvironmentConfig.Create(env);
        return newEnv;
    }

    /// <summary>
    /// Executes the job on the specified environment
    /// </summary>
    /// <param name="env">The environment the job should be executed on</param>
    /// <param name="job">The job to execute</param>
    private static void ExecuteJobOnEnvironment(Environment env, IJob job)
    {
        Console.WriteLine($"You are attempting to run {job.GetType().Name} on {env}.\nPress 'Y' to continue...");
        var keyPressed = Console.ReadKey();
        if (keyPressed.Key != ConsoleKey.Y)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.WriteLine("Aborted by user.\nExiting...");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            return;
        }

        var environment = GetEnv(env);
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            job.Run(environment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine("Program finished.\nPress any key to continue...");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        Console.ReadKey();
    }
}