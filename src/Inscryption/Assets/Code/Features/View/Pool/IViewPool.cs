namespace Code.Features.View.Pool
{
  public interface IViewPool
  {
    IUnityView Get(string viewPath);
    void Put(IUnityView view, string viewPath);
    bool Has(string viewPath);
  }
}