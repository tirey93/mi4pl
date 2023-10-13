#!/usr/bin/env python
# -*- coding=utf-8 -*-

import codecs

from PoReader import PoReader

trans = PoReader("target/efmi.po", want_ids=True).strings

with codecs.open("efmi.tab", "w", "cp1250") as out:
    for i, s in trans:
        out.write("%s\t%s\r\n" % (i, s))

