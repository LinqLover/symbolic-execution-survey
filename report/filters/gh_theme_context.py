#!/usr/bin/env python
"""
Pandoc filter to process images with GitHub theme context for dark/light mode (#gh-dark-mode-only, #gh-light-mode-only).

This filter can be customized by setting variables in the metadata block or on the command line (using -M KEY=VAL). The following variables are supported:
- gh-theme-context-dark-mode: boolean (default: False). If True, the filter will keep images with #gh-dark-mode-only suffix. If False, the filter will keep images with #gh-light-mode-only suffix.
"""

from pandocfilters import Image, toJSONFilter
from pandocxnos import  get_meta, check_bool

def filter_images(key, value, format, meta):
    if key == 'Image':
        attrs, alt, (url, title) = value
        if url.endswith("#gh-dark-mode-only"):
            if is_dark_mode(meta):
                url = url[:-len("#gh-dark-mode-only")]
            else:
                return []
        elif url.endswith("#gh-light-mode-only"):
            if is_dark_mode(meta):
                return []
            else:
                url = url[:-len("#gh-light-mode-only")]
        return Image(attrs, alt, [url, title])

def is_dark_mode(meta):
    name = 'gh-theme-context-dark-mode'
    if name in meta:
        return check_bool(get_meta(meta, name))
    return False

if __name__ == '__main__':
    toJSONFilter(filter_images)
