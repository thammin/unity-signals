namespace UnitySignals
{
    public enum CacheState
    {
        /// <summary>
        /// reactive value is valid, no need to recompute
        /// </summary>
        Clean = 0,

        /// <summary>
        /// reactive value might be stale, check parent nodes to decide whether to recompute
        /// </summary>
        Check = 1,

        /// <summary>
        /// reactive value is invalid, parents have changed, valueneeds to be recomputed
        /// </summary>
        Dirty = 2,
    }
}