import os
import sys

from polyglot.detect import Detector


def DetectLyricLanguage(lyric):
    langs = Detector(lyric)
    lang = langs.languages[0]
    print(lang.code)


if __name__ == '__main__':
    lyricPath = sys.argv[1]
    lyric = ''
    if os.path.exists(lyricPath):
        with open(lyricPath, encoding='utf-8', mode='r') as f:
            for line in f.readlines():
                lyric += line

        DetectLyricLanguage(lyric)
    else:
        print("LyricFile not exists!")
        exit(1)
