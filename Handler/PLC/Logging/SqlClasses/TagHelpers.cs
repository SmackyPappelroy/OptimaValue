namespace OptimaValue
{
    public static class TagHelpers
    {
        public static TagDefinitions GetTagFromId(int id)
        {
            if (id <= 0)
                return null;
            if (TagsToLog.AllLogValues == null)
                return null;
            if (TagsToLog.AllLogValues.Count == 0)
                return null;

            foreach (var tag in TagsToLog.AllLogValues)
            {
                if (id == tag.id)
                    return tag;
            }
            return null;
        }
    }
}
