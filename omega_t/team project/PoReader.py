#!/usr/bin/env python

import re

from babel.messages import pofile

class PoReader:
	def __init__(self, fname, want_ids=False):
		self.buf = open(fname, "rb").read()

		self.buf = self.buf.decode("UTF-8")
		self.buf = self.buf.replace("\r\n", "\n")

		self.strings = []

		trans = re.findall('(?s)msgstr (".*?")\n\n', self.buf)
		orig = re.findall('(?s)msgid (".*?")\nmsgstr', self.buf)
		assert len(trans) == len(orig)

		if want_ids:
			ids = re.findall('(?s)msgctxt "(.*?)"\nmsgid', self.buf)
			assert len(ids) == len(orig)

		for j, i in zip(orig, trans):
			if i.find("Content-Type") != -1:
				continue

			if i == '""':
				i = j

			if i.startswith("\"\"\n"):
				i = i[3:]

			lines = [ pofile.unescape(x) for x in i.splitlines() ]
			lines = [ x.replace("\\n", "") for x in lines ]
			tmp = "".join(lines)
			tmp = re.sub("(?m)^//.*?$", "", tmp)
			self.strings += [ tmp ]

		if want_ids:
			self.strings = zip(ids, self.strings)

