using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Celin.Jira.Response
{
    public class ComparableBase<T> : IEqualityComparer<T>
        where T : Base
    {
        public bool Equals(T x, T y)
        {
            return x.id.Equals(y.id);
        }
        public int GetHashCode(T obj)
        {
            return obj.id.GetHashCode();
        }
    }
    public class Empty { }
    public class AvatarUrls
    {
        [JsonPropertyName("24x24")]
        public string medium { get; set; }
        [JsonPropertyName("16x16")]
        public string small { get; set; }
        [JsonPropertyName("32x32")]
        public string large { get; set; }
    }
    public class Roles
    {
        [JsonPropertyName("atlassian-addons-project-access")]
        public string aapa { get; set; }
        public string Administrator { get; set; }
        public string Viewer { get; set; }
        public string Member { get; set; }
    }
    public class Base
    {
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
    }
    public class User : Base
    {
        public string accountId { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string displayName { get; set; }
        public string emailAddress { get; set; }
        public bool active { get; set; }
        public string timeZone { get; set; }
        public string accountType { get; set; }
    }
    public class IssueType : Base
    {
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }
    public class Project : Base
    {
        public User lead { get; set; }
        public IEnumerable<IssueType> issueTypes { get; set; }
        public Roles roles { get; set; }
        public AvatarUrls avatarUrls { get; set; }
        public string projectTypeKey { get; set; }
    }
    public class StatusCategory
    {
        public int id { get; set; }
        public string self { get; }
        public string key { get; }
        public string name { get; set; }
        public string colorName { get; set; }
    }
    public class Status : Base
    {
        public StatusCategory statusCategory { get; set; }
    }
    public class TaskType : Base
    {
        public bool subtask { get; set; }
        public IEnumerable<Status> statuses { get; set; }
    }
    public class TextContent
    {
        public string text { get; set; }
        public string type { get; set; }
    }
    public class DocContent
    {
        public string type { get; set; }
        public IEnumerable<TextContent> content { get; set; }
    }
    public class DocFormat
    {
        public string type { get; set; }
        public int version { get; set; }
        public IEnumerable<DocContent> content { get; set; }
    }
    public class Timetracking
    {
        public string originalEstimate { get; set; }
        public string remainingEstimate { get; set; }
        public int originalEstimateSeconds { get; set; }
        public int remainingEstimateSeconds { get; set; }
    }
    public class Fields
    {
        public DateTime statuscategorychangedate { get; set; }
        public IssueType issuetype { get; set; }
        public Project project { get; set; }
        public Timetracking timetracking { get; set; }
        public DateTime lastViewed { get; set; }
        public DateTime created { get; set; }
        public Base priority { get; set; }
        public IEnumerable<string> labels { get; set; }
        public DateTime updated { get; set; }
        public Status status { get; set; }
        public DocFormat description { get; set; }
        public string summary { get; set; }
        public User creator { get; set; }
        public User reporter { get; set; }
    }
    public class Issue : Base
    {
        public Fields fields { get; set; }
    }
    public class Comment : Base
    {
        public User author { get; set; }
        public DocFormat body { get; set; }
        public User updateAuthor { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public bool jsdPublic { get; set; }

    }
    public class Issues
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public IEnumerable<Issue> issues { get; set; }
    }
    public class Comments
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public IEnumerable<Comment> comments { get; set; }
    }
    public class Transition
    {
        public string id { get; set; }
        public string name { get; set; }
        public Status to { get; set; }
        public bool hasScreen { get; set; }
        public bool isGlobal { get; set; }
        public bool isInitial { get; set; }
        public bool isConditional { get; set; }
    }
    public class Transitions
    {
        public string expand { get; set; }
        public IEnumerable<Transition> transitions { get; set; }
    }
    public class Label
    {
        public int maxResults { get; set; }
        public int startAt { get; set; }
        public int total { get; set; }
        public bool isLast { get; set; }
        public IEnumerable<string> values { get; set; }
    }
}
