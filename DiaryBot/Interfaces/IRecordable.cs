using System.Collections.Generic;

namespace DiaryBot
{
    public interface IRecordable<TRecord>
    {
        public List<TRecord> Items { get; init; }
        public TRecord? SelectedItem { get; set; }

        public string GetPath();
        public void Add(TRecord newItem);
        public void Update(TRecord updatedItem);
        public void Remove();
    }
}