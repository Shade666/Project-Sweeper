using System.Windows;
using System.Windows.Media;

namespace PKHL.ProjectSweeper.Helpers
{
    /// <summary>
    /// WPF utility methods for visual tree traversal
    /// </summary>
    public static class WpfHelpers
    {
        /// <summary>
        /// Walks up the visual tree to find the first parent of the specified type
        /// </summary>
        /// <typeparam name="T">Type of parent to find</typeparam>
        /// <param name="child">Starting dependency object</param>
        /// <returns>Parent of type T, or null if not found</returns>
        public static T GetVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null)
                return null;

            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;

            return GetVisualParent<T>(parentObject);
        }
    }
}
