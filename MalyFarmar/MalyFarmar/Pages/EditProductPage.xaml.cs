using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class EditProductPage : BaseContentPage
    {
        public EditProductPage(EditProductViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}