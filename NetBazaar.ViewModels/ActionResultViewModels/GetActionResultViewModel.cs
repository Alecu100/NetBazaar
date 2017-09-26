namespace NetBazaar.ViewModels.ActionResultViewModels
{
    public class GetActionResultViewModel<T> : BasicActionResultViewModel
    {
        public T Data { get; set; }
    }
}