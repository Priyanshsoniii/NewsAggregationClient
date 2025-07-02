using System.Text.RegularExpressions;

namespace NewsAggregationClient.Services;

public class CategoryClassificationService
{
    private readonly Dictionary<string, List<string>> _categoryKeywords;

    public CategoryClassificationService()
    {
        _categoryKeywords = new Dictionary<string, List<string>>
        {
            ["business"] = new List<string>
            {
                "stock", "market", "economy", "finance", "investment", "company", "corporate", "earnings", "profit", "revenue",
                "business", "trade", "commerce", "industry", "startup", "entrepreneur", "ceo", "executive", "merger", "acquisition"
            },
            ["entertainment"] = new List<string>
            {
                "movie", "film", "actor", "actress", "celebrity", "hollywood", "entertainment", "music", "singer", "album",
                "concert", "award", "oscar", "grammy", "red carpet", "premiere", "box office", "streaming", "netflix", "disney"
            },
            ["sports"] = new List<string>
            {
                "football", "basketball", "baseball", "soccer", "tennis", "golf", "olympics", "championship", "tournament",
                "player", "team", "coach", "game", "match", "score", "win", "loss", "victory", "defeat", "league", "season"
            },
            ["technology"] = new List<string>
            {
                "tech", "technology", "software", "hardware", "computer", "internet", "digital", "ai", "artificial intelligence",
                "machine learning", "cybersecurity", "blockchain", "crypto", "startup", "app", "mobile", "smartphone", "gadget"
            },
            ["general"] = new List<string>
            {
                "news", "update", "announcement", "report", "information", "general", "public", "community", "society"
            },
            ["politics"] = new List<string>
            {
                "politics", "political", "government", "election", "vote", "campaign", "president", "minister", "parliament",
                "congress", "senate", "policy", "law", "legislation", "democracy", "republic", "liberal", "conservative"
            },
            ["games"] = new List<string>
            {
                "game", "gaming", "video game", "esports", "gamer", "console", "playstation", "xbox", "nintendo", "pc gaming",
                "mobile game", "app store", "steam", "twitch", "streamer", "tournament", "championship"
            },
            ["songs"] = new List<string>
            {
                "song", "music", "singer", "artist", "album", "single", "hit", "chart", "billboard", "radio", "concert",
                "performance", "lyrics", "melody", "rhythm", "genre", "pop", "rock", "hip hop", "country"
            },
            ["festival"] = new List<string>
            {
                "festival", "celebration", "event", "party", "ceremony", "holiday", "tradition", "cultural", "religious",
                "carnival", "parade", "ceremony", "commemoration", "anniversary", "birthday", "wedding"
            },
            ["miscellaneous"] = new List<string>
            {
                "misc", "miscellaneous", "other", "various", "different", "unusual", "unique", "special", "interesting"
            }
        };
    }

    public string ClassifyArticle(string title, string content = "")
    {
        var text = $"{title} {content}".ToLower();
        var scores = new Dictionary<string, int>();

        // Initialize scores
        foreach (var category in _categoryKeywords.Keys)
        {
            scores[category] = 0;
        }

        // Calculate scores based on keyword matches
        foreach (var category in _categoryKeywords.Keys)
        {
            foreach (var keyword in _categoryKeywords[category])
            {
                var pattern = $@"\b{Regex.Escape(keyword)}\b";
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                scores[category] += matches.Count;
            }
        }

        // Find the category with the highest score
        var bestCategory = scores.OrderByDescending(x => x.Value).First();
        
        // If no keywords found, return "general"
        return bestCategory.Value > 0 ? bestCategory.Key : "general";
    }

    public List<string> GetCategories()
    {
        return _categoryKeywords.Keys.ToList();
    }

    public bool IsValidCategory(string category)
    {
        return _categoryKeywords.ContainsKey(category.ToLower());
    }
} 