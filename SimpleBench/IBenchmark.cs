
namespace SimpleBench
{
	public interface IBenchmark
	{
		void SetUp();
		void Run();
		void CleanUp();
	}
}
