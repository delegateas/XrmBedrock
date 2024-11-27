using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseService.Foundation.Exception;

public class CouldNotFetchProfilePictureException : System.Exception
{
    public CouldNotFetchProfilePictureException()
    {
    }

    public CouldNotFetchProfilePictureException(string message)
        : base(message)
    {
    }

    public CouldNotFetchProfilePictureException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}