using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;


public interface IBanWordFilter
{
    void Initialize();
    bool ContainsBlackWord(string str);
    bool ContainsSpecialCharacter(string str);
    string FilterString(string str);
}

public class BanWordFilter : IBanWordFilter
{
    private const int _garbageValue = 8203;

    private FilterWordTree _baneedWordTree = new FilterWordTree();

    public void Initialize()
    {
        //var raws = Grm.GameData.GetDicData<BannedWordRaw>();
        //foreach (var banWordRaw in raws.Values)
        //{
        //    _baneedWordTree.Insert(banWordRaw.Word);
        //}
    }

    /// <summary>
    /// 특수 문자 포함 여부
    /// </summary>
    public bool ContainsSpecialCharacter(string userStr)
    {
        for (int i = 0; i < userStr.Length; ++i)
        {
            var chr = userStr[i];
            if (!char.IsNumber(chr))
            {
                int value = chr;
                if (value == _garbageValue)
                {
                    continue;
                }
            }

            if (char.IsLetterOrDigit(chr) == false)
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsBlackWord(string userStr)
    {
        string str = GetCustomStr(userStr);
        return _baneedWordTree.ContainsBanWord(str);
    }

    public string FilterString(string userStr)
    {
        string removeGarbageStr = RemoveGarbageStr(userStr);
        string checkStr = removeGarbageStr.ToLower();

        char[] result = removeGarbageStr.ToCharArray();
        List<WordInfo> wordInfos = _baneedWordTree.GetContainsInfo(checkStr);
        foreach (var wordInfo in wordInfos)
        {
            for (int i = wordInfo.StartIndex; i <= wordInfo.EndIndex; ++i)
                result[i] = '*';
        }

        return new string(result);
    }


    /// <summary>
    /// InputField로 글자 입력시 특정 상황에서 빈리터널('')이 1~2개 정도 삽입된다(int 변환시 8203).
    /// string의 Trim, IsWithSpace, Replace 계열의 함수로는 제거 및 검출이 안된다.
    /// </summary>
    private string GetCustomStr(string userStr)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < userStr.Length; ++i)
        {
            var chr = userStr[i];
            if (!char.IsNumber(chr))
            {
                int value = chr;
                if (value == _garbageValue)
                {
                    continue;
                }
            }

            chr = char.ToLower(chr);
            stringBuilder.Append(chr);
        }

        return stringBuilder.ToString();
    }

    private string RemoveGarbageStr(string userStr)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < userStr.Length; ++i)
        {
            var chr = userStr[i];
            if (!char.IsNumber(chr))
            {
                int value = chr;
                if (value == _garbageValue)
                    continue;
            }

            stringBuilder.Append(chr);
        }

        return stringBuilder.ToString();
    }

}



public class WordInfo
{
    public WordInfo(int startIndex, int endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
}


public class FilterWordTree
{
    public TreeNode Root { get; set; } = new TreeNode();

    public void Insert(string word)
    {
        var currentNode = Root;

        bool isUpper = false;

        for (int i = 0; i < word.Length; ++i)
        {
            if (char.IsUpper(word[i]))
            {
                isUpper = true;
            }

            int key = word[i] & 0x0f;

            if (currentNode.Childs[key] == null)
            {
                currentNode.Childs[key] = new TreeNode();
            }

            currentNode = currentNode.Childs[key];

            if (i == word.Length - 1)
            {
                currentNode.AddHash(word, isUpper);
            }
        }
    }

    public bool ContainsBanWord(string word)
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < word.Length; ++i)
        {
            var currentNode = Root;

            stringBuilder.Clear();

            for (int j = i; j < word.Length; ++j)
            {
                int key = word[j] & 0x0f;
                if (currentNode.Childs[key] == null)
                {
                    break;
                }

                stringBuilder.Append(word[j]);

                currentNode = currentNode.Childs[key];
                bool contains = currentNode.ContainsHash(stringBuilder.ToString());
                if (contains)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<WordInfo> GetContainsInfo(string word)
    {
        List<WordInfo> wordInfos = new List<WordInfo>();
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < word.Length; ++i)
        {
            var currentNode = Root;

            stringBuilder.Clear();

            for (int j = i; j < word.Length; ++j)
            {
                int key = word[j] & 0x0f;
                if (currentNode.Childs[key] == null)
                {
                    break;
                }

                stringBuilder.Append(word[j]);

                currentNode = currentNode.Childs[key];
                bool contains = currentNode.ContainsHash(stringBuilder.ToString());
                if (contains)
                {
                    wordInfos.Add(new WordInfo(i, j));
                }
            }
        }

        return wordInfos;
    }

    public class TreeNode
    {
        public TreeNode[] Childs = new TreeNode[16];

        private HashSet<int> _hashs;

        public bool ContainsHash(string word)
        {
            if (_hashs == null)
                return false;

            int hash = Animator.StringToHash(word);
            return _hashs.Contains(hash);
        }

        public void AddHash(string word, bool isUpper)
        {
            if (_hashs == null)
                _hashs = new HashSet<int>();

            // ToLower가 문자열을 새로 만드는 함수이기 때문에 대문자가 포함되어있는 경우에만 소문자로 밀어준다.
            if (isUpper)
            {
                word = word.ToLower();
            }

            int hash = Animator.StringToHash(word);
            _hashs.Add(hash);
        }
    }
}
