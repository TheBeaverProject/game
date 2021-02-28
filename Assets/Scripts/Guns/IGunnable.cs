namespace Guns
{
    public interface IGunnable
    {
        void MyInput();
        void Reload();
        void ReloadFinished();
        void Shoot();
        void ResetShot();
    }
}