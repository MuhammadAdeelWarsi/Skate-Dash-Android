// Gives a contract to implement start and end of a time cycle
// Responsible for dealing with objects that exist and being hidden in an environment w.r.t time

public interface ITimeCycle
{
    public void StartCycle();
    public void EndCycle();
}
