namespace CleanOperation
{
    public class CleanAspects
    {
        public virtual void Aspect(Action operation)
        {
            try
            {
                operation();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
        public virtual T Aspect<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
        public virtual async Task<TResult> AspectAsync<TResult>(Func<Task<TResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
        public virtual async Task AspectVoidAsync(Func<Task> operation)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
        }
    }
}
