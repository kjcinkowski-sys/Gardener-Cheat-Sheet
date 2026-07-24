namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Stores user-uploaded images and returns a URL that can be served back to the
/// client. The implementation decides where bytes physically live (local disk,
/// object storage, …); callers only deal in streams and the resulting URL.
/// </summary>
public interface IImageStorage
{
    /// <summary>
    /// Persists the image <paramref name="content"/> and returns a URL that resolves to
    /// it — relative for local disk (e.g. <c>/uploads/plants/ab12….jpg</c>) or absolute for
    /// object storage (e.g. <c>https://pub-xxxx.r2.dev/plants/ab12….jpg</c>).
    /// </summary>
    /// <param name="fileExtension">
    /// The validated extension to store the file under, including the leading dot
    /// (e.g. <c>.jpg</c>). The caller is responsible for validation; the storage
    /// never trusts a client-supplied filename.
    /// </param>
    Task<string> SaveAsync(Stream content, string fileExtension, CancellationToken ct = default);
}
