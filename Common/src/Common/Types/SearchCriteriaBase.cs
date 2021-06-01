using System.Collections.Generic;

namespace Common.Types
{
    public abstract class SearchCriteriaBase 
    {
        public string ResponseGroup { get; set; }

        /// <summary>
        /// Search object type
        /// </summary>
        public virtual string ObjectType { get; set; }

        private IList<string> _objectTypes;
        public virtual IList<string> ObjectTypes
        {
            get
            {
                if (_objectTypes == null && !string.IsNullOrEmpty(ObjectType))
                {
                    _objectTypes = new[] { ObjectType };
                }
                return _objectTypes;
            }
            set
            {
                _objectTypes = value;
            }
        }

        public IList<string> ObjectIds { get; set; }

        /// <summary>
        /// Search phrase
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Property is left for backward compatibility
        /// </summary>
        public string SearchPhrase
        {
            get { return Keyword; }
            set { Keyword = value; }
        }

        /// <summary>
        /// Search phrase language 
        /// </summary>
        public string LanguageCode { get; set; }

        public string Sort { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; } = 20;
    }
}