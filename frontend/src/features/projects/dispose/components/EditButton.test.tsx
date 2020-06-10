import React from 'react';
import renderer from 'react-test-renderer';
import { noop } from 'lodash';
import EditButton from './EditButton';
import { render } from '@testing-library/react';

const getEditButton = (setFormDisabled: Function, canEdit?: boolean, formDisabled?: boolean) => {
  return <EditButton {...{ canEdit, formDisabled, setFormDisabled }} />;
};

describe('Project Dispose Form Edit Button', () => {
  it('Matches Snapshot', () => {
    const component = renderer.create(getEditButton(noop, true, true));
    expect(component.toJSON()).toMatchSnapshot();
  });

  it('The button is disabled if the form is not disabled', () => {
    const { getByText } = render(getEditButton(noop, true, false));
    const editButton = getByText('Edit');
    expect(editButton).toBeTruthy();
    expect(editButton).toBeVisible();
  });

  it('Edit Button is not visible if canEdit false', () => {
    const { queryByText } = render(getEditButton(noop, false, false));
    const editButton = queryByText('Edit');
    expect(editButton).toBeNull();
  });
});