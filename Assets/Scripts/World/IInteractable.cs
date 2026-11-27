namespace AbyssFirstPersonController
{
    public interface IInteractable
    {
        string Prompt { get; }
        bool CanInteract { get; }
        void Interact(PlayerInteractor interactor);
    }
}