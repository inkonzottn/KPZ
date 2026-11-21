namespace OnlineCourses.Core.Documents;

/// <summary>Визначає стани документа реєстрації протягом його життєвого циклу.</summary>
public enum DocumentStatus
{
    /// <summary>Документ створено, але ще не підтверджено.</summary>
    New,
    /// <summary>Документ оплачено.</summary>
    Paid,
    /// <summary>Послугу за документом активовано.</summary>
    Active,
    /// <summary>Склад документа підтверджено.</summary>
    Confirmed,
    /// <summary>Документ скасовано до активації.</summary>
    Cancelled,
    /// <summary>Кошти за активним документом повернено.</summary>
    Refunded
}
