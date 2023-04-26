namespace Mirrors_All_in_One.Enums
{
    public enum PackageManagerType
    {
        /// <summary>
        /// None表示不是任何一个包管理器，若是这个值，则对应页面为“请添加一个包管理器”
        /// </summary>
        None,
        Conda,
        Npm,
        Pip
    }
}