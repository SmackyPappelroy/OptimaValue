using System.Linq;

namespace OptimaValue
{
    public static class TagHelpers
    {
        public static TagDefinitions GetTagFromId(int id)
        {
            if (id <= 0 || TagsToLog.AllLogValues == null || TagsToLog.AllLogValues.Count == 0)
            {
                return null;
            }

            return TagsToLog.AllLogValues.FirstOrDefault(tag => tag.Id == id);
        }

    }
}
