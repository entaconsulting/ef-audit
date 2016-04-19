namespace Dal.Audit
{
    /// <summary>
    /// Interface IAppContext
    /// </summary>
    public interface IAppContext
    {
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        string UserName { get;  }
    }
}
