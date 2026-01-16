using System.ComponentModel;

namespace PlatformKernel.SysException;

public enum ErrorCode
{
    #region Common [E0xxx]

    /// <summary>
    /// Error the system
    /// </summary>
    [Description("Error the system")]
    E0100,

    /// <summary>
    /// Error the database
    /// </summary>
    [Description("Error the database")]
    E0101,

    /// <summary>
    /// Error the app settings
    /// </summary>
    [Description("Error the app settings")]
    E0102,

    /// <summary>
    /// Application user key not found
    /// </summary>
    [Description("Application user key not found")]
    E0103,

    /// <summary>
    /// System is currently processing too many concurrent requests
    /// </summary>
    [Description("System is currently processing too many concurrent requests")]
    E0104

    #endregion
}
