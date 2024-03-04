import re
log_lines = []
with open('all_tests.txt', encoding='utf-8') as f:
    log_lines = f.readlines()
passed_lines = []
for line in log_lines:
    if 'Passed' in line:
        without_date = line.split(' ')[1:]
        passed_lines.append(" ".join(without_date).strip())
parsed = []
for line in passed_lines:    
    m = re.search(r'\[(\d+) (\w+)\]$', line)
    if m is None:        
        #print(f"not_parsed: {line}")
        pass
    else:
        parsed.append({
            'line': str.replace(line,"Passed ", ""),
            'time': int(m.group(1)),
            'time_unit': m.group(2)
        })
minutes = filter(lambda x: x['time_unit'] == 'm', parsed)
print('# MINUTES RANGE:')
for line in minutes:
    print(line['line'])
secs = list(filter(lambda x: x['time_unit'] == 's',  parsed))
secs.sort(key= lambda x: x['time'], reverse=True)
print('# TOP 20 in SECOUNDS RANGE:')
for line in secs[:20]:
    print(line['line'])
