using System;
using System.Collections.Generic;
using System.Web;

namespace Celin.Jira.Request
{
    public class RequestBody { }
    public class EditBody : RequestBody
    {
        public Update update { get; set; }
        public Fields fields { get; set; }
        public DocFormat<ParagraphDoc<TextContent>> body { get; set; }
        public Id transition { get; set; }
    }
    public class SearchBody : RequestBody
    {
        public string jql { get; set; }
        public IEnumerable<string> fields { get; set; }
    }
    public abstract class Base
    {
        public abstract string Path { get; }
        public virtual string Query => string.Empty;
        public RequestBody RequestBody { get; set; }
    }
    public class Project : Base
    {
        protected static readonly string PATH = "project";
        public override string Path => $"{PATH}/{IdOrKey}";
        public string IdOrKey { get; set; }
        public class Statuses : Project
        {
            protected static readonly string STATUSES_PATH = "statuses";
            public override string Path => $"{base.Path}/{STATUSES_PATH}";
        }
    }
    public class Key
    {
        public string key { get; set; }
    }
    public class Id
    {
        public string id { get; set; }
    }
    public class DocFormat<T>
    {
        public int version { get; } = 1;
        public string type { get; } = "doc";
        public IEnumerable<T> content { get; set; }
    }
    public class TextContent
    {
        public string type { get; } = "text";
        public string text { get; set; }
    }
    public class ParagraphDoc<T>
    {
        public string type { get; } = "paragraph";
        public IEnumerable<T> content { get; set; }
    }
    public class Timetracking
    {
        public string originalEstimate { get; set; }
        public string remainingEstimate { get; set; }
    }
    public class FieldUpdate<T>
    {
        public T set { get; set; }
        public T add { get; set; }
        public T edit { get; set; }
        public T remove { get; set; }
    }
    public class Update
    {
        public IEnumerable<FieldUpdate<Timetracking>> timetracking { get; set; }
        public IEnumerable<FieldUpdate<string>> labels { get; set; }
    }
    public class Fields
    {
        public Key project { get; set; }
        public string summary { get; set; }
        public Id issuetype { get; set; }
        public Timetracking timetracking { get; set; }
        public DocFormat<ParagraphDoc<TextContent>> description { get; set; }
        public IEnumerable<string> labels { get; set; }

    }
    public class Issue : Base
    {
        protected static readonly string PATH = "issue";
        public override string Path => $"{PATH}/{IdOrKey}";
        public string IdOrKey { get; set; }
        public class Comment : Issue
        {
            protected static readonly string COMMENT_PATH = "comment";
            public override string Path => $"{base.Path}/{COMMENT_PATH}";
            public class Get : Comment
            {
                public override string Path => $"{base.Path}/{id}";
                public int? id { get; set; }
            }
        }
        public class Transition : Issue
        {
            protected static readonly string TRANSITION_PATH = "transitions";
            public override string Path => $"{base.Path}/{TRANSITION_PATH}";
        }
    }
    public class Label : Base
    {
        protected static readonly string PATH = "label";
        public override string Path => PATH;
        public override string Query
        {
            get
            {
                var q = HttpUtility.ParseQueryString(string.Empty);
                if (startAt.HasValue) q.Add(nameof(startAt), startAt.ToString());
                if (maxResults.HasValue) q.Add(nameof(maxResults), maxResults.ToString());
                return q.ToString();
            }
        }
        public int? startAt { get; set; }
        public int? maxResults { get; set; }
    }
    public class Search : Base
    {
        protected static readonly string PATH = "search";
        public override string Path => PATH;
    }
}
