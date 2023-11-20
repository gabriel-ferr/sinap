namespace JSON;

public struct Qualitative
{
    /// <summary>
    ///     Identificação da coleta.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    ///     Idade da amostra.
    /// </summary>
    public int YearsOld { get; set; }

    /// <summary>
    ///     Identificação da tarefa.
    /// </summary>
    public int Task { get; set; }

    /// <summary>
    ///     Valor qualitativo do teste.
    /// </summary>
    public int Value { get; set; }
}