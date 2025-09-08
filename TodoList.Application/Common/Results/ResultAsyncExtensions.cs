namespace TodoList.Application.Common.Results
{
    public static class ResultAsyncExtensions
    {
        // =========================
        // MAP (project T -> U)
        // =========================

        // Result<T> + projector async
        public static async Task<Result<U>> MapAsync<T, U>(
            this Result<T> r,
            Func<T, Task<U>> projector)
            => r.IsSuccess
                ? Result<U>.Success(await projector(r.Value!).ConfigureAwait(false))
                : Result<U>.Failure(r.Error);

        // Result<T> + projector async + CancellationToken
        public static async Task<Result<U>> MapAsync<T, U>(
            this Result<T> r,
            Func<T, CancellationToken, Task<U>> projector,
            CancellationToken ct)
            => r.IsSuccess
                ? Result<U>.Success(await projector(r.Value!, ct).ConfigureAwait(false))
                : Result<U>.Failure(r.Error);

        // Task<Result<T>> + projector sync
        public static async Task<Result<U>> MapAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, U> projector)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? Result<U>.Success(projector(r.Value!))
                : Result<U>.Failure(r.Error);
        }

        // Task<Result<T>> + projector async
        public static async Task<Result<U>> MapAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, Task<U>> projector)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? Result<U>.Success(await projector(r.Value!).ConfigureAwait(false))
                : Result<U>.Failure(r.Error);
        }

        // Task<Result<T>> + projector async + CancellationToken
        public static async Task<Result<U>> MapAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, CancellationToken, Task<U>> projector,
            CancellationToken ct)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? Result<U>.Success(await projector(r.Value!, ct).ConfigureAwait(false))
                : Result<U>.Failure(r.Error);
        }

        // =========================
        // BIND (flatMap T -> Result<U>)
        // =========================

        // Result<T> + binder async
        public static async Task<Result<U>> BindAsync<T, U>(
            this Result<T> r,
            Func<T, Task<Result<U>>> binder)
            => r.IsSuccess
                ? await binder(r.Value!).ConfigureAwait(false)
                : Result<U>.Failure(r.Error);

        // Result<T> + binder async + CancellationToken
        public static async Task<Result<U>> BindAsync<T, U>(
            this Result<T> r,
            Func<T, CancellationToken, Task<Result<U>>> binder,
            CancellationToken ct)
            => r.IsSuccess
                ? await binder(r.Value!, ct).ConfigureAwait(false)
                : Result<U>.Failure(r.Error);

        // Task<Result<T>> + binder sync
        public static async Task<Result<U>> BindAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, Result<U>> binder)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? binder(r.Value!)
                : Result<U>.Failure(r.Error);
        }

        // Task<Result<T>> + binder async
        public static async Task<Result<U>> BindAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, Task<Result<U>>> binder)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? await binder(r.Value!).ConfigureAwait(false)
                : Result<U>.Failure(r.Error);
        }

        // Task<Result<T>> + binder async + CancellationToken
        public static async Task<Result<U>> BindAsync<T, U>(
            this Task<Result<T>> rTask,
            Func<T, CancellationToken, Task<Result<U>>> binder,
            CancellationToken ct)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? await binder(r.Value!, ct).ConfigureAwait(false)
                : Result<U>.Failure(r.Error);
        }

        // =========================
        // MAP ERROR (Error -> Error)
        // =========================

        public static async Task<Result<T>> MapErrorAsync<T>(
            this Result<T> r,
            Func<Error, Task<Error>> mapper)
            => r.IsSuccess
                ? r
                : Result<T>.Failure(await mapper(r.Error).ConfigureAwait(false));

        public static async Task<Result<T>> MapErrorAsync<T>(
            this Result<T> r,
            Func<Error, CancellationToken, Task<Error>> mapper,
            CancellationToken ct)
            => r.IsSuccess
                ? r
                : Result<T>.Failure(await mapper(r.Error, ct).ConfigureAwait(false));

        public static async Task<Result<T>> MapErrorAsync<T>(
            this Task<Result<T>> rTask,
            Func<Error, Task<Error>> mapper)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? r
                : Result<T>.Failure(await mapper(r.Error).ConfigureAwait(false));
        }

        public static async Task<Result<T>> MapErrorAsync<T>(
            this Task<Result<T>> rTask,
            Func<Error, CancellationToken, Task<Error>> mapper,
            CancellationToken ct)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess
                ? r
                : Result<T>.Failure(await mapper(r.Error, ct).ConfigureAwait(false));
        }

        // =========================
        // MATCH (terminate to R)
        // =========================

        public static Task<R> MatchAsync<T, R>(
            this Result<T> r,
            Func<T, Task<R>> onSuccess,
            Func<Error, Task<R>> onFailure)
            => r.IsSuccess ? onSuccess(r.Value!) : onFailure(r.Error);

        public static Task<R> MatchAsync<T, R>(
            this Result<T> r,
            Func<T, CancellationToken, Task<R>> onSuccess,
            Func<Error, CancellationToken, Task<R>> onFailure,
            CancellationToken ct)
            => r.IsSuccess ? onSuccess(r.Value!, ct) : onFailure(r.Error, ct);

        public static async Task<R> MatchAsync<T, R>(
            this Task<Result<T>> rTask,
            Func<T, Task<R>> onSuccess,
            Func<Error, Task<R>> onFailure)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess ? await onSuccess(r.Value!).ConfigureAwait(false)
                               : await onFailure(r.Error).ConfigureAwait(false);
        }

        public static async Task<R> MatchAsync<T, R>(
            this Task<Result<T>> rTask,
            Func<T, CancellationToken, Task<R>> onSuccess,
            Func<Error, CancellationToken, Task<R>> onFailure,
            CancellationToken ct)
        {
            var r = await rTask.ConfigureAwait(false);
            return r.IsSuccess ? await onSuccess(r.Value!, ct).ConfigureAwait(false)
                               : await onFailure(r.Error, ct).ConfigureAwait(false);
        }

        // =========================
        // (Tuỳ chọn) Non-generic Result tiện dụng
        // =========================

        public static async Task<Result<U>> MapAsync<U>(
            this Result r,
            Func<Task<U>> projector)
            => r.IsSuccess
                ? Result<U>.Success(await projector().ConfigureAwait(false))
                : Result<U>.Failure(r.Error);

        public static async Task<Result<U>> MapAsync<U>(
            this Result r,
            Func<CancellationToken, Task<U>> projector,
            CancellationToken ct)
            => r.IsSuccess
                ? Result<U>.Success(await projector(ct).ConfigureAwait(false))
                : Result<U>.Failure(r.Error);

        public static async Task<Result> BindAsync(
            this Result r,
            Func<Task<Result>> binder)
            => r.IsSuccess ? await binder().ConfigureAwait(false) : Result.Failure(r.Error);

        public static async Task<Result> BindAsync(
            this Result r,
            Func<CancellationToken, Task<Result>> binder,
            CancellationToken ct)
            => r.IsSuccess ? await binder(ct).ConfigureAwait(false) : Result.Failure(r.Error);

        public static Task<R> MatchAsync<R>(
            this Result r,
            Func<Task<R>> onSuccess,
            Func<Error, Task<R>> onFailure)
            => r.IsSuccess ? onSuccess() : onFailure(r.Error);

        public static Task<R> MatchAsync<R>(
            this Result r,
            Func<CancellationToken, Task<R>> onSuccess,
            Func<Error, CancellationToken, Task<R>> onFailure,
            CancellationToken ct)
            => r.IsSuccess ? onSuccess(ct) : onFailure(r.Error, ct);
    }
}
