/*
 * @author Francesco Strada
 */

using Touchables.TokenEngine;

namespace Touchables
{
    public interface ITokenEvents
    {
        void OnTokenPlacedOnScreen(object sender, ApplicationTokenEventArgs e);

        void OnTokenRemovedFromScreen(object sender, ApplicationTokenEventArgs e);

        void OnTokenUpdated(object sender, ApplicationTokenEventArgs e);

    }
}
