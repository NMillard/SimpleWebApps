using System.Collections.Generic;

namespace ReadableCode {
    
    /*
     * Applying wrong naming conventions
     */
    
    public class author {
        private readonly List<string> _pen_names;

        public author(string real_name) {
            this.real_name = real_name;
            _pen_names = new List<string>();
        }

        public string real_name { get; set; }
        public IEnumerable<string> pen_names => this._pen_names;
        public void add_pen_name(string pen_name) => _pen_names.Add(pen_name);
    }
}