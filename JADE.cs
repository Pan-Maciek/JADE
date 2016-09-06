using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaciejKozieja.Lang.JADE {
    public static class JADE {

        private static Stack<Tag> info = new Stack<Tag>();
        private static List<string> selfClosingTags = new List<string>() {
            "area", "base", "br",
            "col", "command", "embed",
            "hr", "img", "input",
            "keygen", "link", "meta",
            "param", "source","track", "wbr"
        };
        public static void Translate(string file) {
            Translate(file, Path.ChangeExtension(file, ".html"));
        }
        public static void Translate(string file, string outfile){
            using (var reader = new StreamReader(file))
            using (var writer = new StreamWriter(outfile)) {
                Tag tag;
                int lastindent = -1;
                writer.WriteLine("<!DOCTYPE html>");
                while (!reader.EndOfStream) {
                    tag = translateToTag(reader.ReadLine());
                    if (tag == null) continue;
                    if (tag.tag == "|") {
                        writer.WriteLine(tag.innerHTML);
                        continue;
                    }
                    if (tag.indent < lastindent && info.Count > 0) {
                        while (info.Count > 0 && info.First().indent != tag.indent) {
                            var temp = info.Pop();
                            if (!temp.selfClosing)
                                writer.WriteLine(tabs(temp.indent) + temp.Close());
                        }
                        if (info.Count > 0) {
                            var temp = info.Pop();
                            if (!temp.selfClosing)
                                writer.WriteLine(tabs(temp.indent) + temp.Close());
                        }
                    }
                    if (tag.indent == lastindent) {
                        var temp = info.Pop();
                        if (!temp.selfClosing)
                            writer.WriteLine(tabs(temp.indent) + temp.Close());
                    }
                    writer.WriteLine(tabs(tag.indent) + tag.Open());
                    info.Push(tag);
                    lastindent = tag.indent;
                }
                for (int i = info.Count; i > 0 ; i--) {
                    var temp = info.Pop();
                    if (!temp.selfClosing)
                        writer.WriteLine(tabs(temp.indent) + temp.Close());
                }
            }
        }

        private static string tabs (int times) {
            var temp = "";
            for (int i = 0; i < times; i++)
                temp += '\t';
            return temp;
        }

        private enum SelectedTag {
            tag,
            id,
            @class
        }

        private static Tag translateToTag(string line) {
            if (string.IsNullOrWhiteSpace(line)) return null;
            var tag = new Tag();
            int i = 0;
            // indent level
            {
                int indent = 0;
                for (; i < line.Length; i++) {
                    if (line[i] == ' ') {
                        indent++;
                    } else if (line[i] == '\t') {
                        indent += 4;
                    } else break;
                }
                tag.indent = indent / 4;
            }
            if (line[i] == '|') {
                tag.innerHTML = line.Substring(i + 1);
                tag.selfClosing = true;
                tag.tag = "|";
                return tag;
            }
            // class id tag
            {
                string temp_tag = "",
                       temp_id = "",
                       temp_class = "",
                       temp_atr = "";
                SelectedTag type = SelectedTag.tag;
                for (; i < line.Length; i++) {
                    if (line[i] == '#') {
                        type = SelectedTag.id;
                        temp_id += ' ';
                    } else if (line[i] == '.') {
                        type = SelectedTag.@class;
                        temp_class += ' ';
                    } else if (line[i] == ' ') {
                        break;
                    } else {
                        switch (type) {
                            case SelectedTag.tag:
                                temp_tag += line[i];
                                break;
                            case SelectedTag.id:
                                temp_id += line[i];
                                break;
                            case SelectedTag.@class:
                                temp_class += line[i];
                                break;
                        }
                    }
                }
                if (i < line.Length) i++;
                try {
                    if (line[i] == '(') {
                        if (line.IndexOf(')', i) != -1) {
                            temp_atr = line.Substring(i + 1, line.IndexOf(')') - i - 1);
                            i = line.IndexOf(')') + 2;
                        }
                    }
                } catch (Exception) { }
                if (!string.IsNullOrWhiteSpace(temp_tag)) {
                    if (selfClosingTags.Contains(temp_tag))
                        tag.selfClosing = true;
                    tag.tag = temp_tag;
                }
                tag.id = temp_id;
                tag.@class = temp_class;
                tag.atributes = temp_atr;
            }
            // innerHTML
            try {
                tag.innerHTML = line.Substring(i);
            } catch (Exception) {
                tag.innerHTML = line.Substring(i - 1);
            }
            return tag;
        }

        private class Tag {
            public string atributes = "",
                          id = "",
                          @class = "",
                          tag = "div";
            public string innerHTML = "";
            public bool selfClosing = false;
            public int indent = 0;
            public string Close() => $"</{tag}>";
            public string Open() {
                if (tag == "|")
                    return innerHTML;
                var builder = new StringBuilder();
                builder.Append($"<{tag}");
                if (!string.IsNullOrWhiteSpace(id))
                    builder.Append($" id=\"{id}\"");
                if (!string.IsNullOrWhiteSpace(@class))
                    builder.Append($" class=\"{@class}\"");
                if (!string.IsNullOrWhiteSpace(atributes)) {
                    builder.Append(' ');
                    builder.Append(atributes);
                }
                builder.Append(">");
                builder.Append(innerHTML);

                return builder.ToString();
            }

        }
    }
}
