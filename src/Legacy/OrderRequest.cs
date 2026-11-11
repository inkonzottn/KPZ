using System.Collections.Generic;

namespace OnlineCourses.src.Legacy;

public sealed record OrderRequest(
    int DocId,
    Member Client,
    IReadOnlyList<RegistrationItem>? Items,
    DocState State,
    string Currency,
    bool SendMail);